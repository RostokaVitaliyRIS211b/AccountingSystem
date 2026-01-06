package auth

import (
	pb "ServerModule/gen/proto/auth"
	mns "ServerModule/gen/proto/mains"
	_ "ServerModule/logging"
	"context"
	"errors"
	"slices"
	"strconv"
	"strings"
	"sync"
	t "time"

	"github.com/golang-jwt/jwt/v5"
	"google.golang.org/grpc"
	"google.golang.org/grpc/codes"
	"google.golang.org/grpc/metadata"
	"google.golang.org/grpc/status"
)

const (
	SecretKey = "KEYCENSORISTHEBESTKEYCENSORISTHEBESTdKEYCENSORISTHEBEST"
)

var RolesForMethods map[string][]int32 = map[string][]int32{
	mns.AccountingSystem_GetAllPermissions_FullMethodName:            {35},
	mns.AccountingSystem_GetItemsByObject_FullMethodName:             {19},
	mns.AccountingSystem_GetAllObjects_FullMethodName:                {23},
	mns.AccountingSystem_GetAllGroupingProps_FullMethodName:          {26},
	mns.AccountingSystem_GetGroupingPropsByItem_FullMethodName:       {},
	mns.AccountingSystem_GetAllTypesOfUnit_FullMethodName:            {32},
	mns.AccountingSystem_GetAllTypesOfItems_FullMethodName:           {},
	mns.AccountingSystem_GetAllRoles_FullMethodName:                  {21},
	mns.AccountingSystem_GetAllUsers_FullMethodName:                  {20},
	mns.AccountingSystem_GetAllNames_FullMethodName:                  {28},
	mns.AccountingSystem_GetAllProducers_FullMethodName:              {30},
	mns.AccountingSystem_GetMetaDataOfItem_FullMethodName:            {24},
	mns.AccountingSystem_GetJournal_FullMethodName:                   {22},
	mns.AccountingSystem_GetAllMetaDataTypes_FullMethodName:          {},
	mns.AccountingSystem_AddItem_FullMethodName:                      {13},
	mns.AccountingSystem_UpdateItem_FullMethodName:                   {15},
	mns.AccountingSystem_RemoveItem_FullMethodName:                   {14},
	mns.AccountingSystem_AddObject_FullMethodName:                    {9},
	mns.AccountingSystem_UpdateObject_FullMethodName:                 {11},
	mns.AccountingSystem_RemoveObject_FullMethodName:                 {10},
	mns.AccountingSystem_AddUser_FullMethodName:                      {1},
	mns.AccountingSystem_UpdateUser_FullMethodName:                   {3},
	mns.AccountingSystem_RemoveUser_FullMethodName:                   {2},
	mns.AccountingSystem_AddRole_FullMethodName:                      {4},
	mns.AccountingSystem_UpdateRole_FullMethodName:                   {6},
	mns.AccountingSystem_RemoveRole_FullMethodName:                   {5},
	mns.AccountingSystem_AddGroupingProperty_FullMethodName:          {27},
	mns.AccountingSystem_AddProducer_FullMethodName:                  {31},
	mns.AccountingSystem_AddNameItem_FullMethodName:                  {29},
	mns.AccountingSystem_AddTypeOfUnit_FullMethodName:                {33},
	mns.AccountingSystem_AddItemMetaData_FullMethodName:              {25},
	mns.AccountingSystem_AddObjectMetaData_FullMethodName:            {37},
	mns.AccountingSystem_AddRecordToJournal_FullMethodName:           {16},
	mns.AccountingSystem_AddGroupingPropertyOfItem_FullMethodName:    {15},
	mns.AccountingSystem_RemoveGroupingPropertyOfItem_FullMethodName: {15},
	mns.AccountingSystem_RemoveItemMetaData_FullMethodName:           {25},
	mns.AccountingSystem_RemoveObjectMetadata_FullMethodName:         {37},
	mns.AccountingSystem_CheckActive_FullMethodName:                  {},
	mns.AccountingSystem_SetGroupingPropertiesOfItem_FullMethodName:  {15},
	mns.AccountingSystem_GetAllObjectMetaData_FullMethodName:         {36},
	mns.AccountingSystem_StartBackup_FullMethodName:                  {38},
	mns.AccountingSystem_GetBackupStatus_FullMethodName:              {38},
}

type CustomClaims struct {
	Username string   `json:"username"`
	Roles    []string `json:"roles"` // например: ["admin", "user"]
	jwt.RegisteredClaims
}

var tokens map[string]t.Time = map[string]t.Time{}
var locker sync.RWMutex

func StartSomeAuthShit() {
	go checkTokensExpiration()
}

func checkTokensExpiration() {
	for {
		for key, val := range tokens {
			if t.Since(val) > 30*t.Minute {
				DeleteToken(key)
			}
		}

		t.Sleep(1 * t.Minute)
	}
}

func AddOrUpdateToken(token string) {
	locker.Lock()
	defer locker.Unlock()
	tokens[token] = t.Now()
}

func DeleteToken(token string) {
	locker.Lock()
	defer locker.Unlock()
	delete(tokens, token)
}

func JWTMiddleware() grpc.UnaryServerInterceptor {
	return func(ctx context.Context, req any, info *grpc.UnaryServerInfo, handler grpc.UnaryHandler) (resp any, err error) {
		md, ok := metadata.FromIncomingContext(ctx)
		if !ok {
			return nil, status.Error(codes.Unauthenticated, "missing metadata")
		}

		if info.FullMethod == pb.AuthService_Authentificate_FullMethodName {
			return handler(ctx, req)
		}

		authHeaders := md["authorization"]
		if len(authHeaders) == 0 {
			return nil, status.Error(codes.Unauthenticated, "missing authorization header")
		}

		tokenString := strings.TrimPrefix(authHeaders[0], "Bearer ")
		if tokenString == authHeaders[0] || tokenString == "" {
			return nil, status.Error(codes.Unauthenticated, "invalid authorization format")
		}

		token, err := jwt.Parse(tokenString, func(t *jwt.Token) (any, error) {
			if _, ok := t.Method.(*jwt.SigningMethodHMAC); !ok {
				return nil, errors.New("unexpected signing method")
			}
			return []byte(SecretKey), nil
		})
		if err != nil || !token.Valid {
			return nil, status.Error(codes.Unauthenticated, "invalid or expired token")
		}

		//Добавить проверку что токен выдан самим сервером

		claims, ok := token.Claims.(jwt.MapClaims)
		if !ok {
			return nil, status.Error(codes.Unauthenticated, "invalid claims format")
		}

		var roles []int32

		if rawRoles, ok := claims["roles"].([]any); ok {
			for _, r := range rawRoles {
				if strR, ok := r.(string); ok {
					if roleNum, err2 := strconv.Atoi(strR); err2 == nil {
						roles = append(roles, (int32)(roleNum))
					}
				}
			}
		}

		var methodRoles = RolesForMethods[info.FullMethod]

		if len(methodRoles) > 0 {
			for _, val := range methodRoles {
				if !slices.Contains(roles, val) {
					return nil, status.Error(codes.PermissionDenied, "Access Denied")
				}
			}
		}

		return handler(ctx, req)
	}
}
