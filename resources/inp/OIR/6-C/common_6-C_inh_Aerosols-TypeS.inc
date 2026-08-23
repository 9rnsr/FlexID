[parameter]
  # ICRP Publ.134 p.53 Table 3.3 / Inhaled particulate materials, Absorption type S
  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0                   # footnote†
  $sb = 0                   # footnote†

  ${C}/fA    = 0.01         # ICRP Publ.134 p.53 Table 3.3 / Inhaled particulate materials, Absorption type S
  ${C}/fA_Re = 0.99 # ≒1   # ICRP Publ.134 p.53 Table 3.3 / footnote††

  ${C}/Blood = 'Blood0'     # Generic model


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_6-C_Generic.inc]
