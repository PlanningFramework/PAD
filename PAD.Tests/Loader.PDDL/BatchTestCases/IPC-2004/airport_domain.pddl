﻿;;
;; PDDL file for the AIPS2000 Planning Competition
;; based on the data generated by the airport simulator Astras.
;;

;; Author: Sebastian Trueg thisshouldbethecurrentdateandtime :(
;; Created with AdlDomainExporter 0.1 by Sebastian Trueg <trueg@informatik.uni-freiburg.de>
;;


(define (domain airport)
(:requirements :adl)

(:types airplane segment direction airplanetype)

(:predicates
                (can-move ?s1 ?s2 - segment ?d - direction)
                (can-pushback ?s1 ?s2 - segment ?d - direction)
                (move-dir ?s1 ?s2 - segment ?d - direction)
                (move-back-dir ?s1 ?s2 - segment ?d - direction)
                (is-blocked ?s1  - segment ?t - airplanetype ?s2 - segment ?d - direction)
                (has-type ?a - airplane ?t - airplanetype)
                (at-segment ?a - airplane ?s - segment)
                (facing ?a - airplane ?d - direction)
                (occupied ?s - segment)
                (blocked ?s - segment ?a - airplane)
                (is-start-runway ?s - segment ?d - direction)
                (airborne ?a - airplane ?s - segment)
                (is-moving ?a - airplane)
                (is-pushing ?a - airplane)
                (is-parked ?a - airplane ?s - segment)
)

(:action move
 :parameters
     (?a - airplane ?t - airplanetype ?d1 - direction ?s1 ?s2  - segment ?d2 - direction)
 :precondition
     (and
                (has-type ?a ?t)
                (is-moving ?a)
                (not (= ?s1 ?s2))
                (facing ?a ?d1)
                (can-move ?s1 ?s2 ?d1)
                (move-dir ?s1 ?s2 ?d2)
                (at-segment ?a ?s1)
                (not 	(exists	(?a1 - airplane)	(and 	(not (= ?a1 ?a))
                                                                (blocked ?s2 ?a1))))
                (forall (?s - segment)	(imply 	(and 	(is-blocked ?s ?t ?s2 ?d2)
                					(not (= ?s ?s1)))
                			(not (occupied ?s))
                		))
                )
 :effect
     (and
                (occupied ?s2)
                (blocked ?s2 ?a)
                (not (occupied ?s1))
                (when 	(not (is-blocked ?s1 ?t ?s2 ?d2))
                	(not (blocked ?s1 ?a)))
                (when 	(not (= ?d1 ?d2))
                	(not (facing ?a ?d1)))
                (not (at-segment ?a ?s1))
                (forall (?s - segment)	(when 	(is-blocked ?s ?t ?s2 ?d2)
                			(blocked ?s ?a)
                		))
                (forall (?s - segment)	(when 	(and	(is-blocked ?s ?t ?s1 ?d1)
                				(not (= ?s ?s2)) 
                				(not (is-blocked ?s ?t ?s2 ?d2))
                			)
                			(not (blocked ?s ?a))
                		))
                (at-segment ?a ?s2)
                (when 	(not (= ?d1 ?d2))
                	(facing ?a ?d2))
                )
)
(:action pushback
 :parameters
(?a - airplane ?t - airplanetype ?d1 - direction ?s1 ?s2  - segment ?d2 - direction)
 :precondition
(and
(has-type ?a ?t)
(is-pushing ?a)
(not (= ?s1 ?s2))
(facing ?a ?d1)
(can-pushback ?s1 ?s2 ?d1)
(move-back-dir ?s1 ?s2 ?d2)
(at-segment ?a ?s1)
(not 	(exists	(?a1 - airplane)	(and 	(not (= ?a1 ?a))
						(blocked ?s2 ?a1))))
(forall (?s - segment)	(imply 	(and 	(is-blocked ?s ?t ?s2 ?d2)
					(not (= ?s ?s1)))
			(not (occupied ?s))
		))
)
 :effect
(and
(occupied ?s2)
(blocked ?s2 ?a)
(forall (?s - segment)	(when 	(is-blocked ?s ?t ?s2 ?d2)
			(blocked ?s ?a)
		))
(forall (?s - segment)	(when 	(and	(is-blocked ?s ?t ?s1 ?d1)
				(not (= ?s ?s2)) 
				(not (is-blocked ?s ?t ?s2 ?d2))
			)
			(not (blocked ?s ?a))
		))
(at-segment ?a ?s2)
(when 	(not (= ?d1 ?d2))
	(facing ?a ?d2))
(not (occupied ?s1))
(when 	(not (is-blocked ?s1 ?t ?s2 ?d2))
	(not (blocked ?s1 ?a)))
(when 	(not (= ?d1 ?d2))
	(not (facing ?a ?d1)))
(not (at-segment ?a ?s1))
)
)
(:action takeoff
 :parameters   (?a - airplane ?s - segment ?d - direction)
 :precondition (and
                (at-segment ?a ?s)
                (facing ?a ?d)
                (is-start-runway ?s ?d)
     )
 :effect (and (not (blocked ?s ?a))
              (not (occupied ?s))
              (not (at-segment ?a ?s))
              (airborne ?a ?s)
              (forall (?s1 - segment)  (when (blocked ?s1 ?a)
                                             (not (blocked ?s1 ?a))
                                       )
              )
         )
)
(:action park
 :parameters (?a - airplane ?t - airplanetype ?s - segment ?d - direction)
 :precondition (and (at-segment ?a ?s)
                    (facing ?a ?d)
                    (is-moving ?a)
               )
 :effect (and (is-parked ?a ?s)
              (not (is-moving ?a))
	        (forall (?ss - segment)	(when 	(and	(is-blocked ?ss ?t ?s ?d)
				                (not (= ?s ?ss))
		                                )
	                                (not (blocked ?ss ?a))
			                )
              )
         )
)
(:action startup
 :parameters (?a - airplane)
 :precondition (is-pushing ?a)
 :effect (and (not (is-pushing ?a))
              (is-moving ?a) )
)
)
