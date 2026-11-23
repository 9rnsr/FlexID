[include:common_50-Sn_parameter.inc]

[parameter]
  # ICRP Publ.151 p.260 Table 28.2 / Ingested materials, All forms

$if parent
  ${Sn}/fA    = 0.02
  ${Sn}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_52-Te.inc]
[include:element_51-Sb.inc]
[include:element_50-Sn.inc]
[include:element_49-In.inc]
[include:element_48-Cd.inc]
