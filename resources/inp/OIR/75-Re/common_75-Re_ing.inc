[include:common_75-Re_parameter.inc]

[parameter]
  # ICRP Publ.151 p.288 Table 32.2 / Ingested materials, All forms

$if parent
  ${Re}/fA    = 0.9
  ${Re}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_76-Os.inc]
[include:element_75-Re.inc]
[include:element_74-W.inc]
[include:element_73-Ta.inc]
