package main

import (
	mains "ServerModule/gen/proto/mains"
	mReal "ServerModule/server/mainService"
	"fmt"
	"reflect"
)

func main() {
	AnalyzeMainServiceCompletion()
}

func AnalyzeMainServiceCompletion() {
	var aser mains.AccountingSystemServer = mains.UnimplementedAccountingSystemServer{}
	var rser mReal.MainService = mReal.MainService{}
	var absType = reflect.TypeOf(aser)
	var realType = reflect.TypeOf(rser)

	for i := 0; i < absType.NumMethod(); i++ {
		method := absType.Method(i)
		_, res := realType.MethodByName(method.Name)
		if res {
			fmt.Println(method.Name)
		}
	}
}
