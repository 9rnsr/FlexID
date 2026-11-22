[include:common_45-Rh_parameter.inc]

[parameter]
  # ICRP Publ.151 p.214 Table 23.2 / Ingested materials, All forms

$if parent
  ${Rh}/fA    = 0.05
  ${Rh}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_45-Rh.inc]
[include:element_44-Ru.inc]
[include:element_43-Tc.inc]
