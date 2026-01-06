package logging

import (
	"fmt"
	"os"
	"path/filepath"
	"sync"
	"time"

	"go.uber.org/zap"
	"go.uber.org/zap/zapcore"
	"gopkg.in/natefinch/lumberjack.v2"
)

// SafeWriter — потокобезопасный переключаемый writer
type SafeWriter struct {
	mu     sync.RWMutex
	writer *lumberjack.Logger
}

func (sw *SafeWriter) Write(p []byte) (n int, err error) {
	sw.mu.RLock()
	defer sw.mu.RUnlock()
	return sw.writer.Write(p)
}

func (sw *SafeWriter) SetWriter(w *lumberjack.Logger) {
	sw.mu.Lock()
	defer sw.mu.Unlock()
	// Закрываем старый writer (flush + close)
	if sw.writer != nil {
		sw.writer.Close()
	}
	sw.writer = w
}

// Global logger (можно сделать через DI или singleton — как удобнее)
var Logger *zap.Logger
var safeWriter = &SafeWriter{}

// InitLogger инициализирует логгер с ежедневной ротацией и 30-дневным TTL
func InitLogger(logDir string) error {
	if err := os.MkdirAll(logDir, 0755); err != nil {
		return fmt.Errorf("не удалось создать директорию логов: %w", err)
	}

	// Инициализация первого логгера (на текущую дату)
	updateLoggerWriter(logDir)

	encoderCfg := zapcore.EncoderConfig{
		TimeKey:     "ts",
		LevelKey:    "level",
		MessageKey:  "msg",
		EncodeLevel: zapcore.CapitalLevelEncoder,
		EncodeTime: func(t time.Time, enc zapcore.PrimitiveArrayEncoder) {
			enc.AppendString(t.Format("2006-01-02 15:04:05"))
		},
		EncodeCaller: zapcore.ShortCallerEncoder,
	}

	core := zapcore.NewCore(
		zapcore.NewConsoleEncoder(encoderCfg),
		zapcore.AddSync(safeWriter),
		zapcore.DebugLevel,
	)

	logger := zap.New(core, zap.AddCaller(), zap.AddStacktrace(zap.ErrorLevel))
	Logger = logger

	// Запускаем фоновую горутину для ежедневного обновления
	go dailyRotation(logDir)

	return nil
}

// updateLoggerWriter создаёт новый lumberjack.Logger на указанную дату
func updateLoggerWriter(logDir string) {
	today := time.Now().Format("2006-01-02")
	filename := filepath.Join(logDir, today+".log")

	lj := &lumberjack.Logger{
		Filename:   filename,
		MaxSize:    100, // MB — можно настроить
		MaxBackups: 0,   // не нужно, т.к. ротация по дате
		MaxAge:     30,  // ← Удалять файлы старше 30 дней — именно то, что нужно!
		Compress:   true,
		LocalTime:  true, // использовать локальное время (не UTC) для имён и ротации
	}

	safeWriter.SetWriter(lj)
}

// dailyRotation переключает writer каждую полночь
func dailyRotation(logDir string) {
	for {
		now := time.Now()
		// Следующая полночь
		nextMidnight := time.Date(now.Year(), now.Month(), now.Day()+1, 0, 0, 0, 0, now.Location())
		durationUntilMidnight := nextMidnight.Sub(now)

		time.Sleep(durationUntilMidnight)
		updateLoggerWriter(logDir)
		Logger.Info("логгер переключён на новый файл", zap.String("file", nextMidnight.Format("2006-01-02")+".log"))
	}
}
