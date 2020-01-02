(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:types typeA typeB)
  (:constants constA constB)
  (:predicates (predA) (predB ?a - typeA) (predC ?a - typeA ?b - typeB))
  (:functions (numFuncA) (numFuncB ?a - typeA) (numFuncC ?a - typeA ?b - typeB) - number
              (objFuncA) - typeA
              (objFuncB ?a - typeA) - typeB
              (objFuncC ?a - typeA ?b - typeB) - object
  )
)
