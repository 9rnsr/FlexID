[include:common_20-Ca_parameter.inc]

[parameter]
  # ICRP Publ.134 p.118 Table 6.2 / Ingested materials, All unspecified forms

$if parent
  ${Ca}/fA    = 0.4
  ${Ca}/fA_Re = fA_MaxValueOfIng
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif

[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_20-Ca.inc]
[include:element_21-Sc.inc]
