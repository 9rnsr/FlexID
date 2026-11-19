[include:common_79-Au_parameter.inc]

[parameter]
  # ICRP Publ.151 p.312 Table 35.2 / Ingested materials, All forms

$if parent
  ${Au}/fA    = 0.1
  ${Au}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fA_MaxValueOfIng
  ${*}/fA_Re  = fA_MaxValueOfIng
$endif


[include:../share/common_intake_ing.inc]

[include:../share/common_HATM+UB.inc]

[include:element_79-Au.inc]
[include:element_78-Pt.inc]
[include:element_77-Ir.inc]
[include:element_76-Os.inc]
[include:element_75-Re.inc]
