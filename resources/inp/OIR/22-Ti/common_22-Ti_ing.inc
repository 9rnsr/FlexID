[include:common_22-Ti_parameter.inc]

[parameter]
  # ICRP Publ.151 p.95 Table 11.2 / Ingested materials, All forms

$if parent
  ${Ti}/fA    = 0.001
  ${Ti}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_22-Ti.inc]
[include:element_21-Sc.inc]
