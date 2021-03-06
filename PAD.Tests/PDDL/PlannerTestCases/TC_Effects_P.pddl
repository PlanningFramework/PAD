(define (problem problemName)
  (:domain domainName)
  (:init (predB)
         (predC)
         (= (numFunc) 6)
         (= (objFunc) constA)
  )
  (:goal (and (predA constA)
              (not (predA constB))
              (not (predB))
              (predC)
           )
  )
)