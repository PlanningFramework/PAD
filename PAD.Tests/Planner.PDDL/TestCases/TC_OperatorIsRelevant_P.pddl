(define (problem problemName)
  (:domain domainName)
  (:goal (and
           (predA)
           (predB)
           (or
             (predC)
             (predD)
           )
           (not (predE))
           (= (numFunc) 5)
           (= (objFunc) constA)
           (predF constA)
           (forall (?a) (and (predG ?a) (predH)))
           (exists (?a) (and (predI ?a) (predJ)))
           (predK constA)
           (predL)
           (not (predM))
         )
  )
)