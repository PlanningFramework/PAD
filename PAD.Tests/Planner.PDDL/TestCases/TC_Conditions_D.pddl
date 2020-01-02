(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:predicates (predA) (predB) (predC) (predD ?a) (predE ?a))
  (:action actionName0
    :parameters (?a ?b)
    :precondition (and
                    (predA)
                    (predB)
                  )
  )
  (:action actionName1
    :parameters (?a ?b)
    :precondition (predE ?a)
    :effect (predE ?b)
  )
  (:action actionName2
    :parameters (?a)
    :precondition (not (predD ?a))
  )
  (:action actionName3
    :parameters ()
    :precondition (and
                    (predA)
                    (not (predB))
					(predC)
                  )
  )
)
