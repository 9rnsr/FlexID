[include:common_55-Cs_parameter.inc]

[parameter]
$if parent
  ${Cs}/fA    = fA_MaxValueOfIng
  ${Cs}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_56-Ba.inc]
[include:element_55-Cs.inc]
