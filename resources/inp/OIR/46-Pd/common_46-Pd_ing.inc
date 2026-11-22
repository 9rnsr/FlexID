[include:common_46-Pd_parameter.inc]

[parameter]
  # ICRP Publ.151 p.220 Table 24.2 / Ingested materials, All forms

$if parent
  ${Pd}/fA    = 0.005
  ${Pd}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_47-Ag.inc]
[include:element_46-Pd.inc]
[include:element_45-Rh.inc]
