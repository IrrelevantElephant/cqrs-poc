# Manual steps

## Create image pull secret for Scaleway registry

```shell
kubectl create secret generic regcred \
    --from-file=.dockerconfigjson=./.docker/config.json \
    --type=kubernetes.io/dockerconfigjson
```
