(define (domain domainName)
  (:requirements :typing :fluents :equality :timed-initial-literals)
  (:types typeA)
  (:constants constA - typeA constB)
  (:predicates (pred ?a))
  (:functions (numFuncA) (numFuncB ?a) - number
              (objFuncA) - typeA
              (objFuncB ?a) - object
  
  )
)