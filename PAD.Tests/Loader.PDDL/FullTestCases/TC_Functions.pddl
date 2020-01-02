(define (domain domainName)
  (:requirements :typing :fluents)
  (:types typeA typeB)
  (:functions (functionA ?a) - (either typeA typeB)
              (functionB ?a - typeA) (functionC ?a ?b - typeA) - object
			  (functionD)
  )
)