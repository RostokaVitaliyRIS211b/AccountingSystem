package main

import (
	bd "ServerModule/BdHandling"
	config "ServerModule/config"
	authpb "ServerModule/gen/proto/auth"
	mainhpb "ServerModule/gen/proto/mains"
	lg "ServerModule/logging"
	auth "ServerModule/server/authService"
	mainS "ServerModule/server/mainService"
	"log"
	"net"
	"os"

	"go.uber.org/zap"
	"google.golang.org/grpc"
)

func main() {
	var try_catch = func() {
		if err := recover(); err != nil {
			log.Fatalln(err)
		}
	}
	defer try_catch()

	err := lg.InitLogger("./logs")
	if err != nil {
		log.Fatalf("Logger wont start")
		os.Exit(1)
	}
	cfg, err := config.LoadFromFile("config.json")
	if err != nil {
		lg.Logger.Error("error with config loading", zap.Error(err))
	}

	bd.OpenNewConnection(cfg.ConnectionStrings.DatabaseConnection)
	bd.DbUser = cfg.DbUser
	bd.DbPassword = cfg.DbPassword
	bd.DbHost = cfg.DbHost
	bd.DbName = cfg.DbName
	bd.DbPort = cfg.DbPort

	s := grpc.NewServer(grpc.UnaryInterceptor(auth.JWTMiddleware()))

	authService := &auth.AuthService{}
	authpb.RegisterAuthServiceServer(s, authService)

	mainService := &mainS.MainService{}
	mainhpb.RegisterAccountingSystemServer(s, mainService)

	auth.StartSomeAuthShit()

	lis, err := net.Listen("tcp", ":"+cfg.Port)
	var f = func() {
		lis.Close()
		s.Stop()
		lg.Logger.Error("Server is Closed")
	}
	defer f()
	if err != nil {
		lg.Logger.Error("failed to listen: ", zap.Error(err))
		os.Exit(1)
	}

	log.Println("AuthService listening on Port: " + cfg.Port)
	lg.Logger.Info("AuthService listening on : ", zap.String("Port", cfg.Port))
	if err := s.Serve(lis); err != nil {
		lg.Logger.Error("failed to serve: ", zap.Error(err))
		os.Exit(1)
	}
}
