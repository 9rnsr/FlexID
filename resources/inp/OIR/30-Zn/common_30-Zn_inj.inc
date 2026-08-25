[include:common_30-Zn_parameter.inc]

[parameter]
$if parent
  ${Zn}/fA    = fA_MaxValueOfIng
  ${Zn}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_31-Ga.inc]
[include:element_30-Zn.inc]
[include:element_29-Cu.inc]
