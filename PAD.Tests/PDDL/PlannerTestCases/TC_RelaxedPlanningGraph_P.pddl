(define (problem problemName)
  (:domain domainName)
  (:init (predA)
         (predB)
         (predC)
         (= total-cost 0)
  )
  (:goal (and
           (predB)
           (predD)
           (predF)
          )
  )
)