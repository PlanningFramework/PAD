(define (problem problemName)
  (:domain domainName)
  (:init (rigidPred))
  (:goal (and
           (and
             (predA)
             (not (predB))
             (or (predC) (predD))
             (predE)
           )
           (and
             (= (objFunc1) constA)
             (= (objFunc2) constB)
           )
           (and
             (= (numFunc1) 3)
             (= (numFunc2) 22.3)
           )
           (and
             (predF constA constA)
             (predF constA constB)
             (predF constB constA)
             (predF constB constB)
           )
           (and
             (predA)
             (predB)
           )
         )
  )
)