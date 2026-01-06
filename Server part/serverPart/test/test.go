package main

import (
	db "ServerModule/BdHandling"
	"fmt"
)

func main() {
	fmt.Println(db.GetUpdateSignleString("\"Users\"", "\"Id\"", "\"Name\"", "\"Password\"", "\"Description\""))
	fmt.Println(db.GetUpdateSignleStringReflect(db.UsersTbName, db.IdColumnName, db.User{}))
}
