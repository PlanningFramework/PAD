(define (problem problemName)
  (:domain domainName)
  (:init (pred constA)
         (predTrue)
         (predZ constA)
         (= (objFunc) constA)
  )
  (:goal (and (pred constA)
              (pred constB)
              (predTrue)
              (predFalse)
              (predZ constA)
              (predZ constB)
              (= (objFunc) constA)
         )
  )
)