(define (domain domainName)
  (:requirements :adl :fluents)
  (:constants constA constB)
  (:functions (numericFunc) - number
              (numericFunc ?a) - number
  )
  (:action actionName
    :parameters ()
    :precondition (and
                    (= (- 70 1) (+ 3 5 (* 7 9) (-2)))
                    (< 3.0 8.00)
                    (> (-7.00) (- 09 ))
                    (>= numericFunc 5)
                    (<= numericFunc 5)
                    (= (+ 1 (numericFunc constA)) (numericFunc constB))
                  )
  )
)
