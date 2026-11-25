[include:common_80-Hg_parameter.inc]

[parameter]
  # ICRP Publ.151 p.327 Table 36.3 / Ingested materials, All forms

$if parent
  ${Hg}/fA    = 0.1
  ${Hg}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_80-Hg_Divalent.inc]
[include:element_79-Au.inc]
[include:element_78-Pt.inc]
[include:element_76-Os.inc]
