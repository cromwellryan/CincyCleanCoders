using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Given_A_Payment
{
    [TestFixture]
    public class When_For_Physical_Product
    {
        [Test]
        public void Generates_Packing_Slip()
        {
            var packingSlipGenerator = new MockPackingSlipGenerator();

            Payment payment = new Payment();
            payment.ProductBeingPurchased = new Product();
            payment.ProductBeingPurchased.IsPhysical = true;

            OrderProcessor processor = new OrderProcessor(packingSlipGenerator);
            PaymentResult result = processor.Process(payment);
            Assert.IsTrue(result.GeneratePackingSlip);
        }
    }

    [TestFixture]
    public class When_For_A_Book
    {
        [Test]
        public void Creates_A_Duplicate_PackingSlip_For_The_RoyaltyDepartment()
        {
            var packingSlipGenerator = new MockPackingSlipGenerator();

            var payment = new Payment() { ProductBeingPurchased = new Book() };
            var processor = new OrderProcessor(packingSlipGenerator);

            var result = processor.Process(payment);

            Assert.AreEqual(2, packingSlipGenerator.PackingSlipCount);
        }
    }
    [TestFixture]
    public class When_For_Membership
    {
        [Test]
        public void should_activate_membership()
        {
         
            var payment = new Payment() { ProductBeingPurchased = new Membership() };
            var processor = new OrderProcessor();

            var result = processor.Process(payment);

            Assert.IsTrue(((Membership)payment.ProductBeingPurchased).IsActivated);
        }

        [Test]
        public void should_upgrade_membership()
        {

            var payment = new Payment() { ProductBeingPurchased = new UpgradeMembership() };
            var processor = new OrderProcessor();

            var result = processor.Process(payment);

            Assert.IsTrue(((UpgradeMembership)payment.ProductBeingPurchased).IsUpgraded);
        }
    }

    public class UpgradeMembership : Membership
    {
        public bool IsUpgraded { get; set; }
    }


    public class MockPackingSlipGenerator
    {
        public int PackingSlipCount { get; private set; }

        public void CreatePackingSlip()
        {
            PackingSlipCount++;
        }

        public void CreateDuplicate()
        {
            PackingSlipCount++;
        }
    }

    public class Payment
    {
        public Product ProductBeingPurchased { get; set; }

    }

    public class Membership : Product
    {
        public Membership()
        {
            IsPhysical = false;
        }

        public bool IsActivated { get; set; }
    }
    public class Book : Product
    {
        public Book()
        {
            IsPhysical = true;
        }
    }
    public class Product
    {
        public bool IsPhysical { get; set; }

    }

    public class OrderProcessor
    {
        private readonly MockPackingSlipGenerator packingSlipGenerator;

        public OrderProcessor(MockPackingSlipGenerator  packingSlipGenerator)
        {
            this.packingSlipGenerator = packingSlipGenerator;
        }

        public OrderProcessor()
        {
        }

        public PaymentResult Process(Payment payment)
        {
            var product = payment.ProductBeingPurchased;

            if (product.IsPhysical)
                packingSlipGenerator.CreatePackingSlip();

            if (product is Book)
                packingSlipGenerator.CreateDuplicate();
            if (product is Membership)
                ((Membership)product).IsActivated = true;

            return new PaymentResult() {GeneratePackingSlip = true};
        }

    }

    public class PaymentResult
    {
        public bool GeneratePackingSlip { get; set; }
    }
}
