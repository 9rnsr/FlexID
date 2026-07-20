[parameter]
  # ICRP Publ.141 p.337 Table 22.11 / Inhaled particulate materials, Absorption type F
  $fr = 1
  $sr = 0.4
  $ss = 0
  $fb = 0.002               # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Pu}/Blood               = 'Blood0'
$else
  ${Pu}/Blood               = 'Blood1'
  ${Pa,Th,Ac,Ra,Rn}/Blood   = 'Blood'
  ${Po}/Blood               = 'Plasma2'
  ${U,Bi,Pb,Tl}/Blood       = 'Plasma'
$endif

$if parent
  ${Pu}/fA    = 5E-4        # ICRP Publ.141 p.337 Table 22.11 / Inhaled particulate materials, Absorption type F
  ${Pu}/fA_Re = 5E-4        # footnote§
$else
  ${Fm}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.454 Table 28.2  / Ingested materials, All compounds
  ${Es}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.446 Table 27.2  / Ingested materials, All compounds
  ${Cf}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.434 Table 26.2  / Ingested materials, All compounds
  ${Bk}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.424 Table 25.2  / Ingested materials, All compounds
  ${Cm}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.408 Table 24.2  / Ingested materials, All compounds
  ${Am}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.382 Table 23.5  / Ingested materials, All compounds
  ${Pu}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.337 Table 22.11 / Ingested materials, Soluble forms
  ${Np}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.262 Table 21.2  / Ingested materials, All chemical forms
  ${U}/fA_MaxValueOfIng  = 0.02         # ICRP Publ.137 p.409 Table 15.2  / Ingested materials, Soluble forms
  ${Pa}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.251 Table 20.2  / Ingested materials, All forms
  ${Th}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.137 p.362 Table 14.2  / Ingested materials, All forms
  ${Ac}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.240 Table 19.2  / Ingested materials, All compounds
  ${Ra}/fA_MaxValueOfIng = 0.2          # ICRP Publ.137 p.325 Table 13.2  / Ingested materials, All forms
  ${Fr}/fA_MaxValueOfIng = 0.99 # ≒1   # ICRP Publ.151 p.363 Table 39.2  / Ingested materials, All forms
  ${At}/fA_MaxValueOfIng = 0.99 # ≒1   # ICRP Publ.151 p.356 Table 38.3  / Ingested materials, All forms
  ${Po}/fA_MaxValueOfIng = 0.1          # ICRP Publ.137 p.279 Table 11.2  / Ingested materials, All chemical forms
  ${Bi}/fA_MaxValueOfIng = 0.05         # ICRP Publ.137 p.253 Table 10.2  / Ingested materials, All forms
  ${Pb}/fA_MaxValueOfIng = 0.2          # ICRP Publ.137 p.220 Table  9.2  / Ingested materials, All forms
  ${Tl}/fA_MaxValueOfIng = 0.99 # ≒1   # ICRP Publ.151 p.344 Table 37.2  / Ingested materials, All forms

  ${^Rn}/fA    = fA_MaxValueOfIng
  ${^Rn}/fA_Re = fA_MaxValueOfIng
$endif
