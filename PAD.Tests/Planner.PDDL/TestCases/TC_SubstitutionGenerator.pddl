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
              constG
  )

  (:action actionName0
    :parameters (?a - typeA)
  )
  (:action actionName1
    :parameters (?a - (either typeA typeB))
  )
  (:action actionName2
    :parameters (?a ?b ?c ?d - typeD)
  )
  (:action actionName3
    :parameters (?a)
  )
  (:action actionName4
    :parameters (?a ?b ?c - object)
  )
  (:action actionName5
    :parameters (?a - typeF ?b - typeB)
  )
)