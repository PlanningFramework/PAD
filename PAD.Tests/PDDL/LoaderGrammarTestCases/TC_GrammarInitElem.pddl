(define (problem problemName)
  (:domain domainName)
  (:init
    (pred cc cc)
    (= func 33)
    (= (func cc) 33)
    (not (pred cc))
    (at 33 (pred cc))
  )
  (:goal (pred))
)