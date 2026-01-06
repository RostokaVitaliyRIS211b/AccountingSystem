package config

import (
	"encoding/json"
	"os"
	"strings"
)

func LoadFromFile(path string) (*Config, error) {
	data, err := os.ReadFile(path)
	if err != nil {
		return nil, err
	}

	var cfg Config
	if err := json.Unmarshal(data, &cfg); err != nil {
		return nil, err
	}
	var values []string
	for val := range strings.SplitSeq(cfg.ConnectionStrings.DatabaseConnection, " ") {
		values = append(values, val[strings.Index(val, "=")+1:])
	}
	cfg.DbPassword = values[4]
	cfg.DbUser = values[3]
	cfg.DbName = values[2]
	cfg.DbPort = values[1]
	cfg.DbHost = values[0]
	return &cfg, nil
}
