[include:common_39-Y_parameter.inc]

[parameter]
$if parent
  ${Y}/fA    = fA_MaxValueOfIng
  ${Y}/fA_Re = fA_MaxValueOfIng     # footnote**
$else
  ${*}/fA    = fA_MaxValueOfIng
  ${*}/fA_Re = fA_MaxValueOfIng
$endif

[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_41-Nb.inc]
[include:element_40-Zr.inc]
[include:element_39-Y.inc]
[include:element_38-Sr.inc]
