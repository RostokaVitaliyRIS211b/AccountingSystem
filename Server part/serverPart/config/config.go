package config

type Config struct {
	ConnectionStrings ConnectionStrings `json:"ConnectionStrings"`
	Port              string            `json:"Port"`
	DbPort            string
	DbHost            string
	DbName            string
	DbUser            string
	DbPassword        string
}

type ConnectionStrings struct {
	DatabaseConnection string `json:"DatabaseConnection"`
}
