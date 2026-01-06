package sessiondatahandling

import (
	"log"
	"sync"
)

var SessionDataForUsers map[string]*SessionData = map[string]*SessionData{}
var locker sync.RWMutex

type SessionData struct {
	ErrorMessage   string
	IsBackupDone   bool
	BackupFilePath string
}

func GetUserData(id string) *SessionData {
	locker.RLock()
	defer stdClosing()
	defer locker.RUnlock()
	elem, ok := SessionDataForUsers[id]

	if ok {
		return elem
	}

	return nil
}

func AddUser(id string) {
	locker.Lock()
	defer stdClosing()
	defer locker.Unlock()

	SessionDataForUsers[id] = new(SessionData)

}

func DeleteUser(id string) {
	locker.Lock()
	defer stdClosing()
	defer locker.Unlock()

	delete(SessionDataForUsers, id)

}

func stdClosing() {
	if err := recover(); err != nil {
		log.Println(err)
	}
}
