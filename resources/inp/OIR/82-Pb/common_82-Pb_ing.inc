[include:common_82-Pb_parameter.inc]

[parameter]
  # ICRP Publ.137 p.220 Table 9.2 / Ingested materials, All forms

$if parent
  ${Pb}/fA    = 0.2
  ${Pb}/fA_Re = fA_MaxValueOfIng    # footnote**
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:common_intake_ing.inc]

[include:common_HATM+UB.inc]

[include:element_84-Po.inc]
[include:element_83-Bi.inc]
[include:element_82-Pb.inc]
[include:element_81-Tl.inc]
[include:element_80-Hg.inc]
[include:element_79-Au.inc]
