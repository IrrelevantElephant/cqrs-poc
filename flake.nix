{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs =
    { self, nixpkgs }:
    let
      system = "x86_64-linux";
      pkgs = import nixpkgs {
        inherit system;
        config = {
          allowUnfree = true;
        };
      };
      allowUnfree = true;
    in
    {
      devShells.${system}.default = pkgs.mkShell {
        name = "dotnet-shell";

        packages = with pkgs; [
          dotnetCorePackages.sdk_10_0-bin
          pnpm
        ];

        shellHook = ''
        '';
      };
    };
}
