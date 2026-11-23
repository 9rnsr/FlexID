[include:common_72-Hf_parameter.inc]

[parameter]
  # ICRP Publ.151 p.268 Table 29.2 / Ingested materials, All forms

$if parent
  ${Hf}/fA    = 0.002
  ${Hf}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_73-Ta.inc]
[include:element_72-Hf.inc]
[include:element_71-Lu.inc]
