{{/*
Expand the name of the chart.
*/}}
{{- define "HyFive.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{- define "HyFive.dbName" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 60 | trimSuffix "-" }}-db
{{- end }}

{{- define "HyFive.adminName" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 57 | trimSuffix "-" }}-admin
{{- end }}


{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "HyFive.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "HyFive.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "HyFive.labels" -}}
helm.sh/chart: {{ include "HyFive.chart" . }}
{{ include "HyFive.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{- define "HyFive.dbLabels" -}}
helm.sh/chart: {{ include "HyFive.chart" . }}
{{ include "HyFive.dbSelectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{- define "HyFive.adminLabels" -}}
helm.sh/chart: {{ include "HyFive.chart" . }}
{{ include "HyFive.adminSelectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "HyFive.selectorLabels" -}}
app.kubernetes.io/name: {{ include "HyFive.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{- define "HyFive.dbSelectorLabels" -}}
app.kubernetes.io/name: {{ include "HyFive.dbName" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{- define "HyFive.adminSelectorLabels" -}}
app.kubernetes.io/name: {{ include "HyFive.adminName" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "HyFive.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "HyFive.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}
