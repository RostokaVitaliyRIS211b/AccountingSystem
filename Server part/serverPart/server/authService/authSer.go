package auth

import (
	db "ServerModule/BdHandling"
	pb "ServerModule/gen/proto/auth"
	gf "ServerModule/generalFunctions"
	lg "ServerModule/logging"
	"context"
	"errors"
	"fmt"
	"time"

	"github.com/golang-jwt/jwt/v5"
	"go.uber.org/zap"
	"google.golang.org/grpc/metadata"
)

type AuthService struct {
	pb.UnimplementedAuthServiceServer
}

func (s *AuthService) Authentificate(ctx context.Context, req *pb.AuthRequest) (*pb.AuthReply, error) {
	defer gf.TryCatch()
	username := req.GetUsername()
	password := req.GetPassword()

	md, ok := metadata.FromIncomingContext(ctx)
	if !ok {
		lg.Logger.Error("Нет метаданных")
		return nil, errors.New("нет метаданных")
	}

	ids := md.Get("Id")
	if len(ids) == 0 {
		lg.Logger.Error("Нет Id хэдэра")
		return nil, errors.New("нет Id хэдэра")
	}
	id := ids[0]

	if len(username) > 0 && len(password) > 0 {
		users, err := db.GetAllUsers()
		if err != nil {
			lg.Logger.Error("Ошибка при получении юзеров из бд ", zap.Error(err))
			return nil, fmt.Errorf("can`t get users from db")
		}
		user, found := gf.From(users).Where(func(x *db.User) bool {
			return x.Name == username && x.Password == password
		}).FirstDefault()

		if !found {
			return nil, fmt.Errorf("invalid credentials")
		}

		perms, errPer := db.GetAllPermissionsOfUser((*user).Id)

		if perms == nil {
			lg.Logger.Error("Ошибка с получением разрешениий пользователя", zap.Error(errPer))
			return nil, fmt.Errorf("invalid credentials")
		}

		claims := CustomClaims{
			Username: username,
			Roles: gf.Select(gf.From(perms), func(p db.Permission) string {
				return gf.ToString(p.Id)
			}).ToList(),
			RegisteredClaims: jwt.RegisteredClaims{
				// Recommended: set subject to user ID or username
				Subject: username,
				// Expires in AccessTokenTTL
				ExpiresAt: nil,
				// Issued at
				IssuedAt: jwt.NewNumericDate(time.Now()),
				// Issuer
				Issuer: "AuthService",
				ID:     id,
			},
		}
		token := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)
		strKey, err := token.SignedString([]byte(SecretKey))
		return &pb.AuthReply{Token: strKey}, err
	}

	return nil, fmt.Errorf("invalid credentials")
}
