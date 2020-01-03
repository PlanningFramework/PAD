(define (domain domainName)
  (:requirements :adl :fluents)
  (:constants constA constB)
  (:functions (numericFunc) (numericFunc ?a) (numericFunc2 ?a) (numericFunc3)
              (numericFunc4) (numericFunc5) (numericFunc6) (numericFunc7) - number
  )
  (:action actionName
    :parameters (?a)
    :precondition ()
    :effect (and
              (assign (numericFunc) 5)
              (assign (numericFunc constA) 3)
              (assign (numericFunc constB) 7)
              (assign (numericFunc2 ?a) 7)
              (assign (numericFunc3) (+ 9 3 (numericFunc2 ?a)))
              (increase (numericFunc4) 3)
              (decrease (numericFunc5) 3)
              (scale-up (numericFunc6) 3)
              (scale-down (numericFunc7) 3)
            )
  )
)