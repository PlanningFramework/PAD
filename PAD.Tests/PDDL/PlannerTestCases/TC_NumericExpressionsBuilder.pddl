(define (domain domainName)
  (:requirements :adl :fluents)
  (:functions (numFunc) - number)
  (:action actionName
    :parameters ()
    :precondition (= 
                    (+
                      (- 5)
                      (+ 2 1)
                      (* 5 9)
                      (/ 2 5)
                      (- 5 8)
                      (numFunc)
                    )
                    0
                  )
  )
)
