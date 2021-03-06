(define (problem problemName)
  (:domain domainName)
  (:requirements :strips :adl)
  (:objects constA constB)
  (:init (pred1 aa) (pred2 bb) (= (funcA bb) 88))
  (:goal (pred3 bb))
  (:constraints (at-most-once (pred aa)))
  (:metric minimize (is-violated prefName))
  (:length (:serial 88) (:parallel 99.9))
)