[include:common_73-Ta_parameter.inc]

[parameter]
  # ICRP Publ.151 p.274 Table 30.2 / Ingested materials, All forms

$if parent
  ${Ta}/fA    = 0.001
  ${Ta}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_74-W.inc]
[include:element_73-Ta.inc]
[include:element_72-Hf.inc]
[include:element_71-Lu.inc]
