(define (domain domainName)
  (:requirements :typing :adl :fluents :action-costs)
  (:constants constA constB)
  (:predicates (predA ?a) (predB) (predC) (predD ?a) (predE ?a))
  (:functions (numFunc) - number
              (total-cost) - number
              (objFunc) - object
  )
  (:action actionName0
    :parameters (?a)
    :precondition (predE ?a)
    :effect (and
              (predA ?a)
              (not (predB))
              (forall (?x) (predD ?x))
              (increase (numFunc) 3)
              (when (predA constB) (assign (objFunc) constB))
            )
  )
  (:action actionName1
    :parameters ()
    :precondition (and
	                (predA constA)
					(predB)
					(predC)
				  )
    :effect (and
	          (predC)
	          (predE constA)
			  (increase (total-cost) 4)
		    )
  )
)
