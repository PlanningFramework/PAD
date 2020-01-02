(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:predicates (predA ?a) (predB ?a))
  (:functions (objFunc) - object)
  (:action actionName
    :parameters ()
    :precondition ()
    :effect (assign objFunc constA)
  )
  (:action actionName2
    :parameters ()
    :precondition ()
    :effect (and
              (forall (?x) (predA ?x))
              (forall (?x) (predB ?x))
            )
  )
)
