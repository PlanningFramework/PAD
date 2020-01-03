(define (domain domainName)
  (:requirements :typing)
  (:types typeA typeB)
  (:action actionA
    :parameters (?a - (either typeA typeB)
                 ?b ?c - typeB
                 ?d - (either typeA)
                 ?e)
	:precondition ()
    :effect ()
  )
)