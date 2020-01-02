(define (domain domainName)
  (:requirements :typing)
  (:types typeA typeB)
  (:constants constA - typeA
              constB constC - (either typeA typeB)
			  constA - typeB
              constD
			  constE
  )
)