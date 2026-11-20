[include:common_31-Ga_parameter.inc]

[parameter]
  # ICRP Publ.151 p.162 Table 17.2 / Ingested materials, All forms

$if parent
  ${Ga}/fA    = 0.001
  ${Ga}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_31-Ga.inc]
[include:element_30-Zn.inc]
