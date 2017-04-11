(function () {
    "use strict";

    angular
        .module("worthItApp", ["ngAnimate"]);

    angular
        .module("worthItApp")
        .controller("products", ["$scope", function ($scope) {
            $scope.productsMouseOver = false;
            $scope.lightShow = false;
        }]);

    angular
        .module("worthItApp")
        .controller("sort", ["$scope", function myfunction($scope) {
            $scope.collapseRating = true;
            $scope.collapsePrice = true;
            $scope.collapseManufacturer = true;
        }]);

    angular
        .module("worthItApp")
        .controller("detail", ["$scope", function ($scope) {
            $scope.detailImage = $(".product-images").children().eq(0).attr("src");

            $scope.changeImage = function (imagePath) {
                $(".product-image-original").attr("src", imagePath);
            };

            $scope.addToCart = function (productId, count) {
                $.post("/cart/add", { id: productId }, function (data, testStatus, jqXHR) {
                    location.reload();
                });
            };
        }]);

    angular
        .module("worthItApp")
        .controller("productList", ["$scope", function ($scope) {
            $scope.addToCart = function (productId, count) {
                $.post("/cart/add", { id: productId }, function (data, testStatus, jqXHR) {
                    location.reload();
                });
            };
        }]);

    angular
    .module("worthItApp")
    .controller("scrollRelative", ["$scope", function ($scope) {
        var totalWidth = $("#relative-items-carousel").width();

        $(document).ready(function () {
            $(".relative-product-image-list").each(function (index) {
                totalWidth -= parseInt($(this).innerWidth());
            });
        });
        
        $(".relative-scroll-to-left").click(function () {
            if ($(".relative-carousel-inner").css("left") < "0%") {
                $(".relative-carousel-inner").animate({
                    left: "+=80%"
                });
            }
            else {
                $(".relative-carousel-inner").animate({
                    left: "+5%"
                }, 200, function () {
                    $(".relative-carousel-inner").animate({
                        left: "0%"
                    });
                });
            }
        });

        
        $(".relative-scroll-to-right").click(function () {
            if ($(".relative-carousel-inner").css("left").slice(0, -2) > totalWidth) {
                $(".relative-carousel-inner").animate({
                    left: "-=80%"
                });
            }
            else {
                $(".relative-carousel-inner").animate({
                    left: "-=5%"
                }, 200, function () {
                    $(".relative-carousel-inner").animate({
                        left: "+=5%"
                    });
                });
            }
        });
    }]);

    angular
        .module("worthItApp")
        .controller("scrollSeller", ["$scope", function ($scope) {
            var totalWidth = $("#seller-items-carousel").width();

            $(document).ready(function () {
                $(".seller-product-image-list").each(function (index) {
                    totalWidth -= parseInt($(this).innerWidth());
                });
            });
        
            $(".seller-scroll-to-left").click(function () {
                if ($(".seller-carousel-inner").css("left") < "0%") {
                    $(".seller-carousel-inner").animate({
                        left: "+=80%"
                    });
                }
                else {
                    $(".seller-carousel-inner").animate({
                        left: "+5%"
                    }, 200, function () {
                        $(".seller-carousel-inner").animate({
                            left: "0%"
                        });
                    });
                }
            });
        

           $(".seller-scroll-to-right").click(function () {
               if ($(".seller-carousel-inner").css("left").slice(0, -2) > totalWidth) {
                   $(".seller-carousel-inner").animate({    
                       left: "-=80%"
                   });
               }
               else {
                   $(".seller-carousel-inner").animate({
                       left: "-=5%"
                   }, 200, function () {
                       $(".seller-carousel-inner").animate({
                           left: "+=5%"
                       });
                   });
               }

               console.log($(".seller-carousel-inner").css("left").slice(0, -2));
           });

           
       }]);


    angular
        .module("worthItApp")
        .controller("cart", ["$scope", function ($scope) {
            $scope.delete = function (productId) {
                $.post("/cart/delete", { id: productId }, function (data, testStatus, jqXHR) {
                    location.reload();
                });

            };

            $scope.changeQuantites = function (productId, inventory) {
                var quantityCart = $("#cartItemsQuantities").val();
                $.post("/cart/refresh", { id: productId, quantity: quantityCart }, function (data, testStatus, jqXHR) {
                    location.reload();
                });
            };
        }]);

    angular
        .module("worthItApp")
        .controller("reviewControl", ["$scope", function ($scope) {
            $scope.editShow = true;
            $scope.showAllDescription = true;

            $scope.deleteReview = function (reviewId) {
                console.log("delete");
                $.post("/product/ReviewDelete", { id: reviewId }, function () {
                    location.reload();
                });
            };

            $scope.editReview = function (reviewId) {
                $scope.editShow = false;
                
            };
        }]);

    angular
        .module("worthItApp")
        .controller("searchBar", ["$scope", "$http", "$location", function ($scope, $http, $location) {
            $scope.searching = false;

            $scope.search = function (searchingWord) {
                if (searchingWord.length > 0) {
                    $scope.searching = true;
                }
                else {
                    $scope.searching = false;
                }
                $http.get("/product/search?query=" + searchingWord).then(function (data, status, headers, confing) {
                    $scope.items = data.data;
                }, function (data, status, headers, config) {
                    console.log("no data");
                });
            };
        }]);

    angular
        .module("worthItApp")
        .controller("checkout", ["$scope", "$http", "$location", function ($scope, $http, $location) {
            $(document).ready(function () {
                $("#ShippingCountry").change(function () {
                    $("#ShippingState").empty();
                    $.post("/checkout/states", { id: this.value }, function (result) {
                        if (result.length === 0) {
                            $("#ShippingState").parents('.form-group').hide();
                        } else {
                            $("#ShippingState").parent('.form-group').show();
                        }
                        $("#ShippingState").append($("<option>", { value: "", text: "" }));
                        $(result).each(function () {
                            $("#ShippingState").append($("<option>", { value: this.Value, text: this.Text }));
                        });

                    });
                });

                $http.post("/checkout/country").then(function (data) {
                    $("#ShippingCountry").append($("<option>", { value: "", text: "" }));
                    $(data.data).each(function () {
                        $("#ShippingCountry").append($("<option>", { value: this.Value, text: this.Text }));
                    });
                });

            });
            
            $("#add-shippingAddress").click(function (event) {
                var street1 = $("#ShippingAddress1").val();
                var street2 = $("#ShippingAddress2").val();
                var city = $("#ShippingCity").val();
                var state = $("#ShippingState").val();
                var zipcode = $("#ZipCode").val();
                var country = $("#ShippingCountry").val();

                $.post("/checkout/ValidateAddress", {
                    street1: street1,
                    street2: street2,
                    city: city,
                    state: state,
                    zipcode: zipcode,
                    country: country
                }).done(function (data) {
                    location.reload();
                });
                event.preventDefault();
                return false;
            });

            
            $(".selectAddress").click(function () {
                var num = $(".selectAddress").index(this);
                $("#ShippingAddress1").val($(".address1").eq(num).children("h4").text());
                $("#ShippingAddress2").val($(".address2").eq(num).children("h4").text());
                $("#ShippingCity").val($(".city").eq(num).children("h4").text());
                $("#ShippingCountry").val($(".country").eq(num).children("h4").text());
                $("#ShippingState").val($(".state").eq(num).children("h4").text());
                $("#ZipCode").val($(".zip").eq(num).children("h4").text());
            });

            $scope.deleteAddress = function (addressId) {
                $http.post("/checkout/deleteAddress", { id: addressId }).then(function () {
                    location.reload();
                });
            };
        }]);

})();