(define (domain domainName)
  (:requirements :adl :fluents)
  (:constants constA constB)
  (:predicates (predA) (predB) (predC) (predD) (predE) (predF ?a ?b) (predZ) (pred1) (pred2))
  (:functions (objFunc1) (objFunc2) (objFunc3) (objFunc4) (objFunc5) (objFunc6) (objFunc7 ?a) - object
              (numFunc1) (numFunc2) (numFunc3) - number
  )
  (:action actionName0
    :parameters ()
    :precondition (predZ)
    :effect (and
              (predA)
              (not (predB))
              (predC)
            )
  )
  (:action actionName1
    :parameters (?a)
    :precondition ()
    :effect (and
              (assign (objFunc1) constA)
              (assign (objFunc2) (objFunc3))
              (assign (objFunc4) (objFunc1))
              (assign (objFunc5) constB)
              (assign (objFunc7 ?a) constB)
            )
  )
  (:action actionName2
    :parameters ()
    :precondition ()
    :effect (and
              (assign (numFunc1) 3)
              (assign (numFunc2) 2)
              (increase (numFunc3) 10)
            )
  )
  (:action actionName3
    :parameters (?a)
    :precondition ()
    :effect (and
              (predA)
              (forall (?b) (predF ?a ?b))
            )
  )
  (:action actionName4
    :parameters ()
    :precondition (predZ)
    :effect (and
              (predC)
              (when (pred1) (predA))
              (when (pred2) (predB))
            )
  )
)
