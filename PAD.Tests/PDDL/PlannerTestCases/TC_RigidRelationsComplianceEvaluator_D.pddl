(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:predicates (predA ?a) (predB ?a))
  (:functions (objFunc) - object)
  (:action actionName
    :parameters (?a)
    :precondition (and
                    (predA ?a)
                    (not (not (predA ?a)))
                    (and (predA ?a) (predB ?a))
                    (or (predA ?a) (not (predB ?a)))
                    (imply (predB ?a) (predA ?a))
                    (forall (?x) (and (predA ?a) (predB ?x)))
                    (exists (?x) (and (predA ?a) (predB ?x)))
                    (and (= (objFunc) constA) (predA ?a))
                  )
  )
)
