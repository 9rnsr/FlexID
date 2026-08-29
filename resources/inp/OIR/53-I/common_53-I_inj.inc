[include:common_53-I_parameter.inc]

[parameter]
$if parent
  ${I}/fA    = 0.99 # ≒1
  ${I}/fA_Re = fA_MaxValueOfIng     # footnote¶
$else
  ${*}/fA    = fA_MaxValueOfIng
  ${*}/fA_Re = fA_MaxValueOfIng
$endif

[include:../share/common_intake_inj.inc]

[include:../share/common_HATM+UB.inc]

[include:element_54-Xe.inc]
[include:element_53-I.inc]
[include:element_52-Te.inc]
[include:element_51-Sb.inc]
