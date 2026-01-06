package generalFunctions

import (
	lg "ServerModule/logging"
	"fmt"
	"iter"
	"slices"
)

//region LINQ methods

type Queryable[T any] struct {
	source iter.Seq[T]
}

// Создаёт Queryable из среза, map, генератора и т.д.
func From[T any](s []T) *Queryable[T] {
	return &Queryable[T]{
		source: slices.Values(s),
	}
}

// Или напрямую из iter.Seq:
func FromSeq[T any](seq iter.Seq[T]) *Queryable[T] {
	return &Queryable[T]{source: seq}
}

func Select[T any, U any](q *Queryable[T], f func(T) U) *Queryable[U] {
	return &Queryable[U]{
		source: func(yield func(U) bool) {
			for v := range q.source {
				if !yield(f(v)) {
					return
				}
			}
		},
	}
}

// Where фильтрует элементы
func (q *Queryable[T]) Where(predicate func(T) bool) *Queryable[T] {
	return &Queryable[T]{
		source: func(yield func(T) bool) {
			for v := range q.source {
				if predicate(v) {
					if !yield(v) {
						return
					}
				}
			}
		},
	}
}

func (q *Queryable[T]) FirstDefault() (*T, bool) {
	var first *T
	found := false
	q.source(func(v T) bool {
		first = &v
		found = true
		return false
	})
	return first, found
}

func (q *Queryable[T]) FirstOrDefault(predicate func(T) bool) (*T, bool) {
	var first *T
	found := false
	q.source(func(v T) bool {
		if predicate(v) {
			first = &v
			found = true
			return false
		}
		return true
	})
	return first, found
}

func (q *Queryable[T]) ToList() []T {
	var result []T
	q.source(func(v T) bool {
		result = append(result, v)
		return true
	})
	return result
}

//endregion

//region General methods

func ToString(x any) string {
	return fmt.Sprint(x)
}

func TryCatch() {
	if r := recover(); r != nil {
		lg.Logger.Error(fmt.Sprint(r))
	}
}

//endregion
