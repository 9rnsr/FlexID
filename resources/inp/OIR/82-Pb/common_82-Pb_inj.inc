[include:common_82-Pb_parameter.inc]

[parameter]
$if parent
  ${Pb}/fA    = fA_MaxValueOfIng
  ${Pb}/fA_Re = fA_MaxValueOfIng    # footnote**
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_84-Po.inc]
[include:element_83-Bi.inc]
[include:element_82-Pb.inc]
[include:element_81-Tl.inc]
[include:element_80-Hg.inc]
[include:element_79-Au.inc]
