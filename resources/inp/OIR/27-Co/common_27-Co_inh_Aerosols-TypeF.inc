[include:common_27-Co_parameter.inc]

[parameter]
  # ICRP Publ.134 p.156 Table 8.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 1
  $ss = 0
  $fb  = 0.03               # footnote*
  $fb0 = 0.00               # footnote*
  $sb = 0.002               # footnote*

$if parent
  ${Co}/fA    = 0.1
  ${Co}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_27-Co.inc]
[include:element_26-Fe.inc]
