(define (domain domainName)
  (:requirements :typing)
  (:types typeA typeB - typeC
          typeC - typeF
		  typeD - (either typeA typeB)
		  typeC - typeE
          typeE
  )
  (:constants constAB - (either typeA typeB)
			  constC - typeC
              constD - typeD
              constF - typeF
              object1
  )
)